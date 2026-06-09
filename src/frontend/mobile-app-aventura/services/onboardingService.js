import AsyncStorage from '@react-native-async-storage/async-storage';
import { updatePreferences } from '../repositories/preferencesRepository';

const ONBOARDING_KEY = '@onboarding_preferences';

export const onboardingService = {
  async save(preferences) {
    const payload = JSON.stringify(preferences);
    // keep local copy
    await AsyncStorage.setItem(ONBOARDING_KEY, payload);

    // try to persist preferences to backend (non-blocking)
    try {
      // map onboarding fields to backend expected shape
      const body = {
        Generos: preferences.categories || [],
        Cidade: preferences.city || null,
      };
      await updatePreferences(body);
    } catch (err) {
      console.warn('Falha ao salvar preferências no backend (onboarding):', err);
    }
  },

  async load() {
    const raw = await AsyncStorage.getItem(ONBOARDING_KEY);
    return raw ? JSON.parse(raw) : null;
  },

  async clear() {
    await AsyncStorage.removeItem(ONBOARDING_KEY);
  },
};
