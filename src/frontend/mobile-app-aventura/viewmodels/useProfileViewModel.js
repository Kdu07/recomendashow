import { useCallback, useState } from 'react';

import { signOut } from '../repositories/authRepository';
import { getProfileData } from '../repositories/profileRepository';

export default function useProfileViewModel(navigation) {
  const profileData = useMemo(
    () => getProfileData(),
    []
  );

  const user = profileData.userProfile || {};
  const menuItems = profileData.profileMenuItems || [];

  const refreshProfile = useCallback(() => {
    setProfileData(getProfileData());
  }, []);

  function handleMenuPress(item) {
    console.log('Abrir item:', item.title);
  }


  async function handleLogout() {
    await signOut();
    navigation.reset({
      index: 0,
      routes: [{ name: 'Auth' }],
    });
  }

  function handleTabPress(tab) {
    console.log('Navegar para:', tab);
  }

  return {
    user,
    menuItems,
    refreshProfile,

    handleMenuPress,
    handleTabPress,
  };
}
