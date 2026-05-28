import AsyncStorage from '@react-native-async-storage/async-storage';

const WISHLIST_STORAGE_KEY = '@aventura:wishlist';

export async function getStoredWishlist() {
  try {
    const storedData = await AsyncStorage.getItem(WISHLIST_STORAGE_KEY);

    if (!storedData) {
      return [];
    }

    return JSON.parse(storedData);
  } catch (error) {
    console.log('Erro ao buscar wishlist:', error);
    return [];
  }
}

export async function saveStoredWishlist(events) {
  try {
    await AsyncStorage.setItem(
      WISHLIST_STORAGE_KEY,
      JSON.stringify(events)
    );

    return true;
  } catch (error) {
    console.log('Erro ao salvar wishlist:', error);
    return false;
  }
}

export async function addEventToWishlist(event) {
  const currentWishlist = await getStoredWishlist();

  const alreadyExists = currentWishlist.some(
    (savedEvent) => savedEvent.id === event.id
  );

  if (alreadyExists) {
    return currentWishlist;
  }

  const updatedWishlist = [...currentWishlist, event];

  await saveStoredWishlist(updatedWishlist);

  return updatedWishlist;
}

export async function removeEventFromWishlist(eventId) {
  const currentWishlist = await getStoredWishlist();

  const updatedWishlist = currentWishlist.filter(
    (event) => event.id !== eventId
  );

  await saveStoredWishlist(updatedWishlist);

  return updatedWishlist;
}

export async function toggleEventInWishlist(event) {
  const currentWishlist = await getStoredWishlist();

  const alreadyExists = currentWishlist.some(
    (savedEvent) => savedEvent.id === event.id
  );

  if (alreadyExists) {
    return removeEventFromWishlist(event.id);
  }

  return addEventToWishlist(event);
}

export async function isEventInWishlist(eventId) {
  const currentWishlist = await getStoredWishlist();

  return currentWishlist.some((event) => event.id === eventId);
}