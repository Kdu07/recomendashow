export const AUTH_MODES = {
  login: 'login',
  register: 'register',
};

export const initialAuthValues = {
  name: '',
  email: '',
  password: '',
  confirmPassword: '',
};

export function createAuthUser({ id, name, email }) {
  return {
    id,
    name,
    email,
  };
}
