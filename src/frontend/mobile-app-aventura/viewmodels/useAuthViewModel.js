import { useMemo, useState } from 'react';

import {
  authenticateUser,
  authenticateWithGoogle,
  createUserAccount,
} from '../repositories/authRepository';
import { AUTH_MODES, initialAuthValues } from '../models/authModels';

function validateAuthValues(mode, values) {
  const errors = {};
  const isLogin = mode === AUTH_MODES.login;

  if (!isLogin && !values.name.trim()) {
    errors.name = 'Informe seu nome completo.';
  }

  if (!values.email.trim()) {
    errors.email = 'Informe seu e-mail.';
  }

  if (!values.password) {
    errors.password = 'Informe sua senha.';
  }

  if (!isLogin && values.password !== values.confirmPassword) {
    errors.confirmPassword = 'As senhas precisam ser iguais.';
  }

  return errors;
}

export function useAuthViewModel() {
  const [mode, setMode] = useState(AUTH_MODES.login);
  const [values, setValues] = useState(initialAuthValues);
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);

  const isLogin = mode === AUTH_MODES.login;

  const content = useMemo(() => {
    if (isLogin) {
      return {
        subtitle: 'Entre para descobrir experiencias culturais unicas e personalizadas para voce.',
        submitLabel: 'Entrar',
      };
    }

    return {
      subtitle: 'Crie sua conta para salvar preferencias e receber recomendacoes personalizadas.',
      submitLabel: 'Criar conta',
    };
  }, [isLogin]);

  function handleModeChange(nextMode) {
    setMode(nextMode);
    setErrors({});
  }

  function updateField(field, value) {
    setValues((currentValues) => ({
      ...currentValues,
      [field]: value,
    }));

    setErrors((currentErrors) => ({
      ...currentErrors,
      [field]: undefined,
    }));
  }

  async function handleSubmit() {
    const validationErrors = validateAuthValues(mode, values);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    setLoading(true);

    try {
      if (isLogin) {
        await authenticateUser({
          email: values.email,
          password: values.password,
        });
      } else {
        await createUserAccount({
          name: values.name,
          email: values.email,
          password: values.password,
        });
      }
    } finally {
      setLoading(false);
    }
  }

  async function handleGoogleAuth() {
    setLoading(true);

    try {
      await authenticateWithGoogle();
    } finally {
      setLoading(false);
    }
  }

  return {
    mode,
    values,
    errors,
    content,
    isLogin,
    loading,
    setMode: handleModeChange,
    updateField,
    handleSubmit,
    handleGoogleAuth,
  };
}
