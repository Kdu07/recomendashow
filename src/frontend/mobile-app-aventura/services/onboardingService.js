import { apiFetch } from './apiClient';
import { getAuthSession } from './tokenStorageService';
import { CATEGORIES, CITIES } from '../models/onboardingData';
import { fetchCategories } from './categoryService';

function getLabelById(items, id) {
  return items.find((item) => item.id === id)?.label ?? id;
}

async function getUserId() {
  const session = await getAuthSession();
  const userId = session?.usuarioId ?? session?.UsuarioId ?? session?.id;
  if (!userId) throw new Error('Sessao de usuario nao encontrada.');
  return userId;
}

export const onboardingService = {
  async save(preferences) {
    const usuarioId = await getUserId();
    const availableCategories = await fetchCategories().catch(() => []);
    const availableGeneros = availableCategories
      .filter((category) => category.id !== 'all')
      .map((category) => category.label);
    const selectedLabels = (preferences.categories || []).map((id) => getLabelById(CATEGORIES, id));
    const generos = selectedLabels.filter((label) =>
      availableGeneros.some((available) => available.toLowerCase() === String(label).toLowerCase())
    );
    const cidade = preferences.city ? getLabelById(CITIES, preferences.city) : null;
    const tags = preferences.frequency ? [preferences.frequency] : [];

    const response = await apiFetch(`/api/Preferencias/${usuarioId}`, {
      method: 'PUT',
      body: JSON.stringify({
        Generos: generos,
        Cidade: cidade,
        Tags: tags,
        ReceberNotificacoes: true,
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      throw new Error(`Erro ao salvar onboarding: ${response.status} ${text}`);
    }

    return response.json();
  },

  async load() {
    const usuarioId = await getUserId();
    const response = await apiFetch(`/api/Preferencias/${usuarioId}`);

    if (!response.ok) {
      const text = await response.text();
      throw new Error(`Erro ao carregar onboarding: ${response.status} ${text}`);
    }

    return response.json();
  },

  async clear() {
    return null;
  },
};
