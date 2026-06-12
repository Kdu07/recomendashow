import { apiFetch } from './apiClient';

export async function fetchPreferences(userId) {
  if (!userId) return null;

  try {
    const response = await apiFetch(`/api/Preferencias/${userId}`);

    if (!response.ok) {
      if (response.status === 404) {
        return null;
      }
      throw new Error('Erro ao buscar preferências');
    }

    return await response.json();
  } catch (error) {
    console.warn('Falha ao buscar preferências do usuário:', error);
    return null;
  }
}
