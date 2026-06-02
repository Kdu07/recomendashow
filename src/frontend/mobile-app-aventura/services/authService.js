import { createAuthUser } from '../models/authModels';

// Configure a base URL apropriada para seu ambiente.
// Se você rodar o backend na mesma máquina e testar no emulador/dispositivo, ajuste conforme necessário.
const API_BASE = global.__DEV__
  ? 'http://172.20.10.3:5253'
  : 'http://localhost:5253';

function waitForMockResponse() {
  return new Promise((resolve) => {
    setTimeout(resolve, 400);
  });
}

export async function loginWithEmail(credentials) {
  try {
    const res = await fetch(`${API_BASE}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(credentials),
    });

    if (!res.ok) {
      const text = await res.text();
      throw new Error(`Login failed: ${res.status} ${text}`);
    }

    const data = await res.json();
    return data;
  } catch (error) {
    console.error('loginWithEmail error:', error);
    throw error;
  }
}

export async function registerWithEmail({ name, email }) {
    await waitForMockResponse();

  return {
    user: createAuthUser({
      id: 'new-user',
      name,
      email,
    }),
    token: 'mock-register-token',
  };
}

export async function loginWithGoogle() {
  await waitForMockResponse();

  return {
    user: MOCK_USER,
    token: 'mock-google-token',
  };
}
