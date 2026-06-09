import { apiFetch } from './apiClient';
import { getAuthSession } from './tokenStorageService';

function normalizeProfileResponse(data) {
  return {
    id: data.usuarioId ?? data.UsuarioId ?? data.id,
    name: data.nome ?? data.Nome ?? data.name,
    email: data.email ?? data.Email ?? data.emailAddress,
    avatar: data.avatar ?? data.avatarUrl ?? data.AvatarUrl ?? null,
  };
}

export async function getUserInfo() {
  try {
    const session = await getAuthSession();

    const userId = session?.usuarioId;
    if (!userId) throw new Error('Sessão de usuário não encontrada.');
    const response = await apiFetch(`/api/Usuario/${userId}`);

    if (!response.ok) {
      const text = await response.text();
      throw new Error(`Erro ${response.status}: ${text}`);
    }

    const json = await response.json();
    return normalizeProfileResponse(json);
  } catch (error) {
    console.error('Erro ao buscar usuário:', error);
    throw error;
  }
}

export async function updateUserInfo(payload) {
  try {
    const session = await getAuthSession();
    const userId = session?.usuarioId;
    if (!userId) throw new Error('Sessão de usuário não encontrada.');

    const response = await apiFetch(`/api/Usuario/${userId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const text = await response.text();
      throw new Error(`Erro ao atualizar usuário: ${response.status} ${text}`);
    }

    const json = await response.json();
    return normalizeProfileResponse(json);
  } catch (error) {
    console.error('Erro ao atualizar usuário:', error);
    throw error;
  }
}