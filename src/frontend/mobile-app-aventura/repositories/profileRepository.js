import {userProfile} from '../data/mockProfileData';
import {profileMenuItems} from '../constants/profileMenuItems';

export function getProfileData() {
  return {
    userProfile,
    profileMenuItems,
  };
}

export function updateProfileData(profileUpdates) {
  Object.assign(userProfile, {
    ...profileUpdates,
    name: profileUpdates.name?.trim() || userProfile.name,
    email: profileUpdates.email?.trim() || userProfile.email,
    birthDate: profileUpdates.birthDate?.trim() || userProfile.birthDate,
    cep: profileUpdates.cep?.trim() || userProfile.cep,
    street: profileUpdates.street?.trim() || userProfile.street,
    number: profileUpdates.number?.trim() || userProfile.number,
    complement: profileUpdates.complement?.trim() || '',
    neighborhood: profileUpdates.neighborhood?.trim() || userProfile.neighborhood,
    city: profileUpdates.city?.trim() || userProfile.city,
    avatarIcon: profileUpdates.avatarIcon || userProfile.avatarIcon,
  });

  return userProfile;
}
