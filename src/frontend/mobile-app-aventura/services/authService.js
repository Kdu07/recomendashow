import { API_BASE_URL } from '../constants/api';

function calculateAge(dateString) {
  const [day, month, year] = dateString.split('/').map(Number);
  const birth = new Date(year, month - 1, day);
  const today = new Date();
  let age = today.getFullYear() - birth.getFullYear();
  const hasHadBirthday =
    today.getMonth() > birth.getMonth() ||
    (today.getMonth() === birth.getMonth() && today.getDate() >= birth.getDate());
  return hasHadBirthday ? age : age - 1;
}

async function post(path, body) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });

  const json = await response.json();

  if (!response.ok) {
    throw new Error(json.detail || json.title || `Erro ${response.status}`);
  }

  return json;
}

export async function loginWithEmail({ email, password }) {
  const data = await post('/api/auth/login', {
    Email: email,
    Senha: password,
  });
  return {
    user: { id: data.usuarioId, name: data.nome, email: data.email },
  };
}

export async function registerWithEmail({
  name,
  email,
  password,
  birthDate,
  cep,
  street,
  number,
  complement,
  neighborhood,
  city,
}) {
  const data = await post('/api/auth/register', {
    Nome: name,
    Email: email,
    Senha: password,
    Idade: calculateAge(birthDate),
    Cep: cep,
    Logradouro: street,
    Numero: number,
    Complemento: complement || null,
    Bairro: neighborhood,
    Cidade: city,
  });
  return {
    user: { id: data.usuarioId, name: data.nome, email: data.email },
  };
}

export async function loginWithGoogle() {
  throw new Error('Login com Google ainda não implementado.');
}
