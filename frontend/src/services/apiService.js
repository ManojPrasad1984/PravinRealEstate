const API_BASE_URL = (process.env.REACT_APP_API_BASE_URL || 'http://localhost:7071').replace(/\/$/, '');

async function send(path, options = {}) {
  try {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      headers: { 'Content-Type': 'application/json' },
      ...options
    });

    const raw = await response.text();
    const data = raw ? JSON.parse(raw) : {};

    if (!response.ok) {
      throw new Error(data.message || data.detail || 'API request failed');
    }

    return data;
  } catch (error) {
    if (error instanceof SyntaxError) {
      throw new Error('Invalid API response format.');
    }

    if (error.name === 'TypeError') {
      throw new Error('Cannot reach API. Check REACT_APP_API_BASE_URL and CORS settings.');
    }

    throw error;
  }
}

export const apiService = {
  getAgents: () => send('/api/agents'),
  createOrder: () => send('/api/orders', { method: 'POST' }),
  verifyPayment: (payload) => send('/api/payments/verify', { method: 'POST', body: JSON.stringify(payload) })
};
