import { useEffect, useState } from 'react';

import { signOut } from '../repositories/authRepository';
import { getProfileData } from '../repositories/profileRepository';

export default function useProfileViewModel(navigation) {
  const [user, setUser] = useState({});
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;

    async function load() {
      setLoading(true);
      const data = await getProfileData();
      if (!mounted) return;
      setUser(data.userProfile || {});
      setMenuItems(data.profileMenuItems || []);
      setLoading(false);
    }

    load();

    return () => {
      mounted = false;
    };
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

  return {
    user,
    menuItems,

    isLoading,

    handleMenuPress,
    handleEditProfile,
    handleLogout,
    handleTabPress,
  };
}