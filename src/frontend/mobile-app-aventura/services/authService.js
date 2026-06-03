import { createAuthUser } from '../models/authModels';

const MOCK_USER = createAuthUser({
  id: 'user-1',
  name: 'Usuario Aventura',
  email: 'usuario@aventura.com',
});

function waitForMockResponse() {
  return new Promise((resolve) => {
    setTimeout(resolve, 400);
  });
}

export async function loginWithEmail({ email }) {
  await waitForMockResponse();

  return {
    user: createAuthUser({
      ...MOCK_USER,
      email,
    }),
    token: 'mock-auth-token',
  };
}

export async function registerWithEmail({
  name,
  email,
  age,
  cep,
  street,
  number,
  complement,
  neighborhood,
  city,
}) {
  await waitForMockResponse();

  return {
    user: createAuthUser({
      id: 'new-user',
      name,
      email,
      age,
      cep,
      street,
      number,
      complement,
      neighborhood,
      city,
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
