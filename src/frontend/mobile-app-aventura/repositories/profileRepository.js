import { userProfile as mockUserProfile } from '../data/mockProfileData';
import { profileMenuItems } from '../constants/profileMenuItems';
import { getUserInfo } from '../services/userProfileService';
import { getPreferences } from './preferencesRepository';
import { getInterestData } from './interestRepository';

export async function getProfileData() {
  try {
    const userProfile = await getUserInfo();
    // try to load preferences to show selected genres in the profile menu
    let menuItems = profileMenuItems;
    try {
      const prefs = await getPreferences();
      if (prefs && prefs.generos) {
        const generosArr = Array.isArray(prefs.generos) ? prefs.generos : prefs.Generos ?? prefs.generos ?? [];

        const interestData = getInterestData();
        const mapped = generosArr.map((g) => {
          const found = interestData.interests.find((i) => i.id === g);
          return found ? found.label : g;
        });

        const subtitle = mapped.length > 0 ? mapped.join(', ') : 'Nenhuma';
        menuItems = profileMenuItems.map((it) => it.id === 'preferences' ? { ...it, subtitle } : it);
      }
    } catch (err) {
      console.warn('Erro ao carregar preferências para o perfil:', err);
    }

    return {
      userProfile,
      profileMenuItems: menuItems,
    };
  } catch (error) {
    // fallback to mock data on error
    console.warn('Falling back to mock profile data:', error);
    return {
      userProfile: mockUserProfile,
      profileMenuItems,
    };
  }
}

export function updateProfileData(profileUpdates) {
  Object.assign(mockUserProfile, {
    ...profileUpdates,
    name: profileUpdates.name?.trim() || mockUserProfile.name,
    email: profileUpdates.email?.trim() || mockUserProfile.email,
    birthDate: profileUpdates.birthDate?.trim() || mockUserProfile.birthDate,
    cep: profileUpdates.cep?.trim() || mockUserProfile.cep,
    street: profileUpdates.street?.trim() || mockUserProfile.street,
    number: profileUpdates.number?.trim() || mockUserProfile.number,
    complement: profileUpdates.complement?.trim() || '',
    neighborhood: profileUpdates.neighborhood?.trim() || mockUserProfile.neighborhood,
    city: profileUpdates.city?.trim() || mockUserProfile.city,
    avatarIcon: profileUpdates.avatarIcon || mockUserProfile.avatarIcon,
  });

  return mockUserProfile;
}
