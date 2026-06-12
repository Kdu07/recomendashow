import { apiFetch } from '../services/apiClient';
import { getAuthSession } from '../services/tokenStorageService';

async function getUserIdFromSession() {
  const session = await getAuthSession();
  const userId = session?.usuarioId ?? session?.UsuarioId ?? session?.id;
  if (!userId) throw new Error('Sessão de usuário não encontrada.');
  return userId;
}

export async function getPreferences() {
  try {
    const usuarioId = await getUserIdFromSession();
    const response = await apiFetch(`/api/Preferencias/${usuarioId}`);
    if (!response.ok) {
      const text = await response.text();
      throw new Error(`Erro ao buscar preferências: ${response.status} ${text}`);
    }

    return await response.json();
  } catch (error) {
    console.error('getPreferences error:', error);
    return null;
  }
}

export async function updatePreferences(payload) {
  try {
    const usuarioId = await getUserIdFromSession();
    const response = await apiFetch(`/api/Preferencias/${usuarioId}`, {
      method: 'PUT',
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const text = await response.text();
      throw new Error(`Erro ao atualizar preferências: ${response.status} ${text}`);
    }

    return await response.json();
  } catch (error) {
    console.error('updatePreferences error:', error);
    return null;
  }
}
