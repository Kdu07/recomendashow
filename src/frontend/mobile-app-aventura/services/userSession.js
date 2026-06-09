let currentUserId = null;

export function setCurrentUserId(id) {
  currentUserId = id;
}

export function getCurrentUserId() {
  return currentUserId;
}

export function clearCurrentUserId() {
  currentUserId = null;
}
