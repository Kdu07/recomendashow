import { userProfile as mockUserProfile } from '../data/mockProfileData';
import { profileMenuItems } from '../constants/profileMenuItems';
import { getUserInfo } from '../services/userProfileService';

export async function getProfileData() {
  try {
    const userProfile = await getUserInfo();
    return {
      userProfile,
      profileMenuItems,
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
