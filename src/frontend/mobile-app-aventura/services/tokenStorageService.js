import AsyncStorage from '@react-native-async-storage/async-storage';

const AUTH_TOKEN_KEY = '@aventura:auth_token';
const AUTH_SESSION_KEY = '@aventura:auth_session';

export async function saveAuthToken(token) {
  if (!token) {
    return false;
  }

  try {
    await AsyncStorage.setItem(AUTH_TOKEN_KEY, token);
    return true;
  } catch (error) {
    console.log('Erro ao salvar token:', error);
    return false;
  }
}

export async function getAuthToken() {
  try {
    return await AsyncStorage.getItem(AUTH_TOKEN_KEY);
  } catch (error) {
    console.log('Erro ao buscar token:', error);
    return null;
  }
}

export async function saveAuthSession(session) {
  if (!session) {
    return false;
  }

  try {
    await AsyncStorage.setItem(AUTH_SESSION_KEY, JSON.stringify(session));
    return true;
  } catch (error) {
    console.log('Erro ao salvar sessão:', error);
    return false;
  }
}

export async function getAuthSession() {
  try {
    const raw = await AsyncStorage.getItem(AUTH_SESSION_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch (error) {
    console.log('Erro ao buscar sessão:', error);
    return null;
  }
}

export async function clearAuthStorage() {
  try {
    await AsyncStorage.multiRemove([AUTH_TOKEN_KEY, AUTH_SESSION_KEY]);
    return true;
  } catch (error) {
    console.log('Erro ao limpar autenticação:', error);
    return false;
  }
}

export async function saveAuthData({ token, usuarioId, nome, email }) {
  const tokenSaved = await saveAuthToken(token);
  const sessionSaved = await saveAuthSession({
    usuarioId,
    nome,
    email,
  });

  return tokenSaved && sessionSaved;
}
