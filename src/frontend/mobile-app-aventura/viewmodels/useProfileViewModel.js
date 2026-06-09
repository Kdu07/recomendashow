import { useCallback, useState } from 'react';

import { getProfileData } from '../repositories/profileRepository';
import { removeUserAccount } from '../repositories/authRepository';
import { getCurrentUserId, clearCurrentUserId } from '../services/userSession';

export default function useProfileViewModel() {
  const [profileData, setProfileData] = useState(() => getProfileData());
  const [deletingAccount, setDeletingAccount] = useState(false);

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

  async function handleDeleteAccount() {
    const userId = getCurrentUserId();
    if (!userId) {
      throw new Error('Sessão inválida. Faça login novamente.');
    }

    setDeletingAccount(true);
    try {
      await removeUserAccount(userId);
      clearCurrentUserId();
    } finally {
      setDeletingAccount(false);
    }
  }

  return {
    user,
    menuItems,
    refreshProfile,
    deletingAccount,

    handleMenuPress,
    handleTabPress,
    handleDeleteAccount,
  };
}
