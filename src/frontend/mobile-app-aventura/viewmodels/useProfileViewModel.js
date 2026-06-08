import { useCallback, useState } from 'react';

import { getProfileData } from '../repositories/profileRepository';

export default function useProfileViewModel() {
  const [profileData, setProfileData] = useState(() => getProfileData());

  const user = profileData.userProfile || {};
  const menuItems = profileData.profileMenuItems || [];

  const refreshProfile = useCallback(() => {
    setProfileData(getProfileData());
  }, []);

  function handleMenuPress(item) {
    console.log('Abrir item:', item.title);
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
