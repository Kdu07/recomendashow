import { useCallback, useEffect, useState } from 'react';

import { removeUserAccount, signOut } from '../repositories/authRepository';
import { getProfileData } from '../repositories/profileRepository';
import { clearCurrentUserId } from '../services/userSession';
import { getAuthSession } from '../services/tokenStorageService';

export default function useProfileViewModel(navigation) {
  const [user, setUser] = useState({});
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [deletingAccount, setDeletingAccount] = useState(false);

  const refreshProfile = useCallback(async () => {
    setLoading(true);
    try {
      const data = await getProfileData();
      setUser(data.userProfile || {});
      setMenuItems(data.profileMenuItems || []);
    } catch (err) {
      console.warn('Erro ao carregar perfil:', err);
      setUser({});
      setMenuItems([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refreshProfile();
  }, [refreshProfile]);

  function handleMenuPress(item) {
    console.log('Abrir item:', item.title);
  }

  function handleEditProfile() {
    navigation.navigate('EditProfile');
  }

  async function handleLogout() {
    await signOut();
    clearCurrentUserId();
    navigation.reset({
      index: 0,
      routes: [{ name: 'Auth' }],
    });
  }

  async function handleDeleteAccount() {
    const session = await getAuthSession();
    const userId = session?.usuarioId;
    if (!userId) {
      throw new Error('Sessao invalida. Faca login novamente.');
    }

    setDeletingAccount(true);
    try {
      await removeUserAccount(userId);
      await signOut();
      clearCurrentUserId();
    } finally {
      setDeletingAccount(false);
    }
  }

  return {
    user,
    menuItems,
    deletingAccount,
    isLoading: () => loading,
    refreshProfile,
    handleMenuPress,
    handleEditProfile,
    handleLogout,
    handleDeleteAccount,
  };
}
