import AsyncStorage from '@react-native-async-storage/async-storage';

const ONBOARDING_KEY = '@onboarding_preferences';

export const onboardingService = {
  async save(preferences) {
    const payload = JSON.stringify(preferences);
    await AsyncStorage.setItem(ONBOARDING_KEY, payload);
  },

  async load() {
    const raw = await AsyncStorage.getItem(ONBOARDING_KEY);
    return raw ? JSON.parse(raw) : null;
  },

  async clear() {
    await AsyncStorage.removeItem(ONBOARDING_KEY);
  },
};
