import { useMemo } from 'react';

import { getProfileData } from '../repositories/profileRepository';

export default function useProfileViewModel() {
  const profileData = useMemo(
    () => getProfileData(),
    []
  );

  const user = profileData.userProfile || {};
  const menuItems =
    profileData.profileMenuItems || [];

  function handleMenuPress(item) {
    console.log('Abrir item:', item.title);
  }

  function handleEditProfile() {
    console.log('Editar perfil');
  }

  function handleLogout() {
    console.log('Logout');
  }

  function handleTabPress(tab) {
    console.log('Navegar para:', tab);
  }

  return {
    user,
    menuItems,

    handleMenuPress,
    handleEditProfile,
    handleLogout,
    handleTabPress,
  };
}