const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

async function send(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options
  });

  const data = await response.json();
  if (!response.ok) throw new Error(data.message || 'API request failed');
  return data;
}

export const apiService = {
  getAgents: () => send('/api/agents'),
  createOrder: () => send('/api/orders', { method: 'POST' }),
  verifyPayment: (payload) => send('/api/payments/verify', { method: 'POST', body: JSON.stringify(payload) })
};
