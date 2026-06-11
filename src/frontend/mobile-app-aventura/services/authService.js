import { API_BASE, apiFetch } from './apiClient';
import { clearAuthStorage, saveAuthData } from './tokenStorageService';

function normalizeAuthResponse(data) {
  return {
    usuarioId: data.usuarioId ?? data.UsuarioId,
    nome: data.nome ?? data.Nome,
    email: data.email ?? data.Email,
    token: data.token ?? data.Token,
  };
}

export async function persistAuthResponse(data) {
  const auth = normalizeAuthResponse(data);

  if (!auth.token) {
    throw new Error('Resposta de autenticação sem token.');
  }

  await saveAuthData(auth);
  return auth;
}

async function postAuth(path, payload) {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`Autenticação falhou: ${res.status} ${text}`);
  }

  return res.json();
}

export async function loginWithEmail({ email, password }) {
  const data = await postAuth('/api/auth/login', {
    Email: email,
    Senha: password,
  });
  const auth = await persistAuthResponse(data);

  return {
    user: {
      id: auth.usuarioId,
      name: auth.nome,
      email: auth.email,
    },
    token: auth.token,
  };
}

export async function logout() {
  await clearAuthStorage();
}

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

function toIsoDate(dateString) {
  const [day, month, year] = String(dateString ?? '').split('/').map(Number);

  if (!day || !month || !year) {
    return null;
  }

  return `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
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
  consentimentoAceito,
}) {
  const data = await postAuth('/api/auth/register', {
    Nome: name,
    Email: email,
    Senha: password,
    Idade: calculateAge(birthDate),
    DataNascimento: toIsoDate(birthDate),
    Cep: cep,
    Logradouro: street,
    Numero: number,
    Complemento: complement || null,
    Bairro: neighborhood,
    Cidade: city,
    ConsentimentoAceito: consentimentoAceito,
  });

  const auth = await persistAuthResponse(data);
  return {
    user: {
      id: auth.usuarioId,
      name: auth.nome,
      email: auth.email,
    },
    token: auth.token,
  };
}

export async function deleteUserAccount(userId) {
  const response = await apiFetch(`/api/Usuario/${userId}`, {
    method: 'DELETE',
  });

  if (!response.ok) {
    const json = await response.json().catch(() => ({}));
    throw new Error(json.detail || json.title || `Erro ${response.status}`);
  }
}

export async function loginWithGoogle() {
  throw new Error('Login com Google ainda não implementado.');
}
