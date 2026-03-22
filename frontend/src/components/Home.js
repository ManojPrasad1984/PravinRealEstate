import React from 'react';
import { Link } from 'react-router-dom';

export default function Home() {
  return (
    <div className="row g-4">
      <section className="col-12">
        <div className="p-5 bg-light rounded-3 border">
          <h1 className="display-6">Invest in Future. Enter the Lucky Draw.</h1>
          <p className="lead mb-3">Join Wealthline Lucky Draw and win exciting prizes after successful payment.</p>
          <Link to="/apply" className="btn btn-primary">Apply Now</Link>
        </div>
      </section>

      <section className="col-md-6">
        <div className="card h-100">
          <div className="card-body">
            <h5 className="card-title">Scheme</h5>
            <p className="card-text">Single payment entry with instant card generation after payment verification.</p>
          </div>
        </div>
      </section>

      <section className="col-md-6">
        <div className="card h-100">
          <div className="card-body">
            <h5 className="card-title">Layout Plan</h5>
            <p className="card-text">Transparent lucky draw process with validated participant records.</p>
          </div>
        </div>
      </section>

      <section className="col-12">
        <div className="card">
          <div className="card-body">
            <h5 className="card-title">Contact</h5>
            <p className="mb-0">Need help? Reach us at support@wealthline.example or +91-9000000000.</p>
          </div>
        </div>
      </section>
    </div>
  );
}
