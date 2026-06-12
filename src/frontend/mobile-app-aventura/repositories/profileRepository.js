import { profileMenuItems } from '../constants/profileMenuItems';
import { getPreferences } from './preferencesRepository';
import { getUserInfo, updateUserInfo } from '../services/userProfileService';

function buildMenuItems(preferences) {
  if (!preferences?.Generos && !preferences?.generos) {
    return profileMenuItems;
  }

  const generos = preferences.Generos ?? preferences.generos ?? [];
  const subtitle = generos.length > 0 ? generos.join(', ') : 'Nenhuma preferencia salva';

  return profileMenuItems.map((item) =>
    item.id === 'preferences'
      ? { ...item, subtitle }
      : item
  );
}

function calculateAgeFromBirthDate(value) {
  const [day, month, year] = String(value ?? '').split('/').map(Number);

  if (!day || !month || !year) {
    return null;
  }

  const birth = new Date(year, month - 1, day);
  const today = new Date();
  let age = today.getFullYear() - birth.getFullYear();
  const hadBirthday =
    today.getMonth() > birth.getMonth() ||
    (today.getMonth() === birth.getMonth() && today.getDate() >= birth.getDate());

  return hadBirthday ? age : age - 1;
}

function toIsoDate(value) {
  const [day, month, year] = String(value ?? '').split('/').map(Number);

  if (!day || !month || !year) {
    return null;
  }

  return `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
}

export async function getProfileData() {
  const [userProfile, preferences] = await Promise.all([
    getUserInfo(),
    getPreferences(),
  ]);

  return {
    userProfile,
    profileMenuItems: buildMenuItems(preferences),
  };
}

export async function updateProfileData(profileUpdates) {
  const payload = {
    Nome: profileUpdates.name?.trim(),
    Email: profileUpdates.email?.trim(),
    Idade: calculateAgeFromBirthDate(profileUpdates.birthDate),
    DataNascimento: toIsoDate(profileUpdates.birthDate),
    Cep: profileUpdates.cep?.trim() || null,
    Logradouro: profileUpdates.street?.trim() || null,
    Numero: profileUpdates.number?.trim() || null,
    Complemento: profileUpdates.complement?.trim() || null,
    Bairro: profileUpdates.neighborhood?.trim() || null,
    Cidade: profileUpdates.city?.trim() || null,
  };

  return updateUserInfo(payload);
}
