import React, { useEffect, useState } from 'react';
import { apiService } from '../services/apiService';

const initialState = {
  fullName: '',
  address: '',
  mobileNumber: '',
  aadhaarNumber: '',
  prizeChoice: '',
  agentId: ''
};

export default function ApplyForm() {
  const [form, setForm] = useState(initialState);
  const [agents, setAgents] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [successCard, setSuccessCard] = useState('');

  useEffect(() => {
    apiService.getAgents().then(setAgents).catch(() => setError('Unable to load agents.'));
  }, []);

  const onChange = (e) => setForm((old) => ({ ...old, [e.target.name]: e.target.value }));

  const validate = () => {
    if (!form.fullName.trim()) return 'Full name is required.';
    if (!/^[6-9][0-9]{9}$/.test(form.mobileNumber)) return 'Invalid mobile number.';
    if (!/^[0-9]{12}$/.test(form.aadhaarNumber)) return 'Invalid Aadhaar number.';
    return '';
  };

  const payNow = async (e) => {
    e.preventDefault();
    setError('');

    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }

    try {
      setLoading(true);
      const order = await apiService.createOrder();

      const razorpay = new window.Razorpay({
        key: order.key || process.env.REACT_APP_RAZORPAY_KEY,
        amount: order.amount * 100,
        currency: 'INR',
        order_id: order.orderId,
        name: 'Wealthline Lucky Draw',
        description: 'Lucky draw entry payment',
        handler: async function (paymentResponse) {
          try {
            const payload = {
              entry: {
                fullName: form.fullName,
                address: form.address,
                mobileNumber: form.mobileNumber,
                aadhaarNumber: form.aadhaarNumber,
                prizeChoice: form.prizeChoice,
                agentId: form.agentId || null
              },
              ...paymentResponse
            };

            const verifyResult = await apiService.verifyPayment(payload);
            setSuccessCard(verifyResult.cardNumber);
            setForm(initialState);
          } catch (verifyError) {
            setError(verifyError.message);
          }
        },
        prefill: {
          name: form.fullName,
          contact: form.mobileNumber
        },
        theme: { color: '#0d6efd' }
      });

      razorpay.open();
    } catch (apiError) {
      setError(apiError.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="row justify-content-center">
      <div className="col-lg-8">
        <div className="card">
          <div className="card-body">
            <h4 className="mb-3">Lucky Draw Application</h4>
            {error && <div className="alert alert-danger">{error}</div>}

            <form onSubmit={payNow}>
              <div className="row g-3">
                <div className="col-md-6">
                  <label className="form-label">Full Name</label>
                  <input className="form-control" name="fullName" value={form.fullName} onChange={onChange} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label">Mobile Number</label>
                  <input className="form-control" name="mobileNumber" value={form.mobileNumber} onChange={onChange} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label">Aadhaar Number</label>
                  <input className="form-control" name="aadhaarNumber" value={form.aadhaarNumber} onChange={onChange} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label">Agent</label>
                  <select className="form-select" name="agentId" value={form.agentId} onChange={onChange}>
                    <option value="">Select Agent</option>
                    {agents.map((agent) => (
                      <option key={agent.id} value={agent.id}>{agent.displayName}</option>
                    ))}
                  </select>
                </div>
                <div className="col-12">
                  <label className="form-label">Address</label>
                  <textarea className="form-control" name="address" value={form.address} onChange={onChange} rows="2" />
                </div>
                <div className="col-12">
                  <label className="form-label">Prize Choice (optional)</label>
                  <input className="form-control" name="prizeChoice" value={form.prizeChoice} onChange={onChange} />
                </div>
              </div>

              <button type="submit" className="btn btn-primary mt-4" disabled={loading}>
                {loading ? 'Processing...' : 'Pay and Submit'}
              </button>
            </form>
          </div>
        </div>
      </div>

      {successCard && (
        <div className="modal show d-block" tabIndex="-1" role="dialog">
          <div className="modal-dialog modal-dialog-centered" role="document">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Payment Successful</h5>
                <button type="button" className="btn-close" onClick={() => setSuccessCard('')}></button>
              </div>
              <div className="modal-body">
                <p>Your lucky draw entry is confirmed.</p>
                <p className="mb-0"><strong>Card Number:</strong> {successCard}</p>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
