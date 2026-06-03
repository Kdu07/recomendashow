export const AUTH_MODES = {
  login: 'login',
  register: 'register',
};

export const initialAuthValues = {
  name: '',
  email: '',
  birthDate: '',
  cep: '',
  street: '',
  number: '',
  complement: '',
  neighborhood: '',
  city: '',
  password: '',
  confirmPassword: '',
};

export function createAuthUser({
  id,
  name,
  email,
  birthDate,
  cep,
  street,
  number,
  complement,
  neighborhood,
  city,
}) {
  return {
    id,
    name,
    email,
    birthDate,
    cep,
    street,
    number,
    complement,
    neighborhood,
    city,
  };
}
