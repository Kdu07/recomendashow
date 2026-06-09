import { useCallback, useState, useEffect } from 'react';

import { signOut } from '../repositories/authRepository';
import { getProfileData } from '../repositories/profileRepository';
import { removeUserAccount } from '../repositories/authRepository';
import { getCurrentUserId, clearCurrentUserId } from '../services/userSession';

export default function useProfileViewModel(navigation) {
  const [user, setUser] = useState({});
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [deletingAccount, setDeletingAccount] = useState(false);

  useEffect(() => {
    let mounted = true;

    async function load() {
      setLoading(true);
      try {
        const data = await getProfileData();
        if (!mounted) return;
        setUser(data.userProfile || {});
        setMenuItems(data.profileMenuItems || []);
      } catch (err) {
        console.warn('Erro ao carregar perfil:', err);
        if (!mounted) return;
        setUser({});
        setMenuItems([]);
      } finally {
        if (mounted) setLoading(false);
      }
    }

    load();

    return () => {
      mounted = false;
    };
  }, []);

  const refreshProfile = useCallback(async () => {
    setLoading(true);
    try {
      const data = await getProfileData();
      setUser(data.userProfile || {});
      setMenuItems(data.profileMenuItems || []);
    } catch (err) {
      console.warn('Erro ao atualizar perfil:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  function handleMenuPress(item) {
    console.log('Abrir item:', item.title);
  }

  function handleEditProfile() {
    console.log('Editar perfil');
  }

  async function handleLogout() {
    await signOut();
    navigation.reset({
      index: 0,
      routes: [{ name: 'Auth' }],
    });
  }

  function isLoading() {
    return loading;
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
    isLoading,
    deletingAccount,

    handleMenuPress,
    handleEditProfile,
    handleLogout,
    handleTabPress,
    handleDeleteAccount,
  };
}
