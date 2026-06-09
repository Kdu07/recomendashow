const API_BASE_URL = 'http://localhost:5253/api';

async function request(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      Accept: 'application/json',
      ...(options.body ? { 'Content-Type': 'application/json' } : {}),
      ...options.headers,
    },
    ...options,
  });

  if (!response.ok) {
    let message = `Erro HTTP ${response.status}`;

    try {
      const payload = await response.json();
      message = payload.detail || payload.title || message;
    } catch {
      // Mantem a mensagem padrao quando a API nao retorna JSON.
    }

    throw new Error(message);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export const apiClient = {
  get: (path) => request(path),
  post: (path, body) => request(path, {
    method: 'POST',
    body: body ? JSON.stringify(body) : undefined,
  }),
  delete: (path) => request(path, { method: 'DELETE' }),
};
