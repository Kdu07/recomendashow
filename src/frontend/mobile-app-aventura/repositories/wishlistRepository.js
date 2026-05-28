import {
  getStoredWishlist,
  saveStoredWishlist,
  addEventToWishlist,
  removeEventFromWishlist,
  toggleEventInWishlist,
  isEventInWishlist,
} from '../services/wishlistStorageService';

export async function getWishlistData() {
  const events = await getStoredWishlist();

  return {
    events,
  };
}

export async function saveWishlistData(events) {
  return saveStoredWishlist(events);
}

export async function addWishlistEvent(event) {
  return addEventToWishlist(event);
}

export async function removeWishlistEvent(eventId) {
  return removeEventFromWishlist(eventId);
}

export async function toggleWishlistEvent(event) {
  return toggleEventInWishlist(event);
}

export async function checkEventIsSaved(eventId) {
  return isEventInWishlist(eventId);
}