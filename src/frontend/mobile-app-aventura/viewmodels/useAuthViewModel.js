import { useEffect, useMemo, useRef, useState } from 'react';

import {
  authenticateUser,
  authenticateWithGoogle,
  createUserAccount,
} from '../repositories/authRepository';
import { AUTH_MODES, initialAuthValues } from '../models/authModels';
import { setCurrentUserId } from '../services/userSession';
import { formatCep, lookupAddressByCep, onlyCepDigits } from '../services/viaCepService';

function parseBrazilianDate(value) {
  const [day, month, year] = String(value ?? '').split('/').map(Number);

  if (!day || !month || !year) {
    return null;
  }

  const date = new Date(year, month - 1, day);
  const isValidDate = (
    date.getFullYear() === year
    && date.getMonth() === month - 1
    && date.getDate() === day
  );

  return isValidDate ? date : null;
}

function getAgeFromDate(date) {
  const today = new Date();
  let age = today.getFullYear() - date.getFullYear();
  const hasHadBirthdayThisYear = (
    today.getMonth() > date.getMonth()
    || (today.getMonth() === date.getMonth() && today.getDate() >= date.getDate())
  );

  if (!hasHadBirthdayThisYear) {
    age -= 1;
  }

  return age;
}

function validateAuthValues(mode, values, scope = 'all') {
  const errors = {};
  const isLogin = mode === AUTH_MODES.login;
  const cepDigits = String(values.cep ?? '').replace(/\D/g, '');
  const shouldValidateAccount = scope === 'all' || scope === 'account';
  const shouldValidateAddress = scope === 'all' || scope === 'address';

  if (!isLogin && shouldValidateAccount && !values.name.trim()) {
    errors.name = 'Informe seu nome completo.';
  }

  if (!isLogin && shouldValidateAccount) {
    const birthDateValue = String(values.birthDate ?? '').trim();
    const birthDate = parseBrazilianDate(birthDateValue);

    if (!birthDateValue) {
      errors.birthDate = 'Informe sua data de nascimento.';
    } else if (!birthDate) {
      errors.birthDate = 'Informe uma data válida no formato DD/MM/AAAA.';
    } else if (getAgeFromDate(birthDate) < 13) {
      errors.birthDate = 'Você precisa ter pelo menos 13 anos.';
    }
  }

  if (!isLogin && shouldValidateAddress) {
    if (!cepDigits) {
      errors.cep = 'Informe seu CEP.';
    } else if (cepDigits.length !== 8) {
      errors.cep = 'Informe um CEP válido com 8 dígitos.';
    }

    if (!String(values.street ?? '').trim()) {
      errors.street = 'Informe sua rua.';
    }

    if (!String(values.neighborhood ?? '').trim()) {
      errors.neighborhood = 'Informe seu bairro.';
    }

    if (!String(values.city ?? '').trim()) {
      errors.city = 'Informe sua cidade.';
    }
  }

  if (shouldValidateAccount && !values.email.trim()) {
    errors.email = 'Informe seu e-mail.';
  } else if (shouldValidateAccount && !isLogin && !values.email.includes('@')) {
    errors.email = 'Informe um e-mail válido.';
  }

  if (shouldValidateAccount && !values.password) {
    errors.password = 'Informe sua senha.';
  } else if (shouldValidateAccount && !isLogin && values.password.length < 6) {
    errors.password = 'A senha deve ter no mínimo 6 caracteres.';
  }

  if (shouldValidateAccount && !isLogin && values.password !== values.confirmPassword) {
    errors.confirmPassword = 'As senhas precisam ser iguais.';
  }

  if (!isLogin && scope === 'all' && !values.consentimentoAceito) {
    errors.consentimentoAceito = 'Você precisa aceitar a política de privacidade.';
  }

  return errors;
}

export function useAuthViewModel(navigation) {
  const [mode, setMode] = useState(AUTH_MODES.login);
  const [values, setValues] = useState(initialAuthValues);
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [cepLookupLoading, setCepLookupLoading] = useState(false);
  const lastCepLookupRef = useRef('');

  const isLogin = mode === AUTH_MODES.login;

  useEffect(() => {
    if (isLogin) {
      return undefined;
    }

    const cepDigits = onlyCepDigits(values.cep);

    if (cepDigits.length !== 8) {
      lastCepLookupRef.current = '';
      setCepLookupLoading(false);
      setValues((currentValues) => {
        if (!currentValues.street && !currentValues.neighborhood && !currentValues.city) {
          return currentValues;
        }

        return {
          ...currentValues,
          street: '',
          neighborhood: '',
          city: '',
        };
      });
      return undefined;
    }

    if (lastCepLookupRef.current === cepDigits) {
      return undefined;
    }

    let cancelled = false;

    const timeoutId = setTimeout(async () => {
      lastCepLookupRef.current = cepDigits;
      setCepLookupLoading(true);

      try {
        const address = await lookupAddressByCep(cepDigits);

        if (cancelled) {
          return;
        }

        setValues((currentValues) => {
          if (onlyCepDigits(currentValues.cep) !== cepDigits) {
            return currentValues;
          }

          return {
            ...currentValues,
            street: address.street,
            neighborhood: address.neighborhood,
            city: address.city,
          };
        });

        setErrors((currentErrors) => ({
          ...currentErrors,
          cep: undefined,
          street: undefined,
          neighborhood: undefined,
          city: undefined,
        }));
      } catch (error) {
        if (cancelled) {
          return;
        }

        setValues((currentValues) => {
          if (onlyCepDigits(currentValues.cep) !== cepDigits) {
            return currentValues;
          }

          return {
            ...currentValues,
            street: '',
            neighborhood: '',
            city: '',
          };
        });

        setErrors((currentErrors) => ({
          ...currentErrors,
          cep: error.message || 'Não foi possível consultar o CEP.',
        }));
      } finally {
        if (!cancelled) {
          setCepLookupLoading(false);
        }
      }
    }, 350);

    return () => {
      cancelled = true;
      clearTimeout(timeoutId);
    };
  }, [isLogin, values.cep]);

  const content = useMemo(() => {
    if (isLogin) {
      return {
        subtitle: 'Entre para descobrir experiências culturais únicas e personalizadas para você.',
        submitLabel: 'Entrar',
      };
    }

    return {
      subtitle: 'Crie sua conta para salvar preferências e receber recomendações personalizadas.',
      submitLabel: 'Criar conta',
    };
  }, [isLogin]);

  function handleModeChange(nextMode) {
    setMode(nextMode);
    setErrors({});
  }

  function updateField(field, value) {
    const nextValue = field === 'cep' ? formatCep(value) : value;

    setValues((currentValues) => ({
      ...currentValues,
      [field]: nextValue,
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
      return {
        ok: false,
        reason: 'validation',
      };
    }

    setLoading(true);

    try {
      if (isLogin) {
        const result = await authenticateUser({
          email: values.email,
          password: values.password,
        });
        setCurrentUserId(result?.user?.id ?? null);
        navigation.replace('MainTabs');
      } else {
        const result = await createUserAccount({
          name: values.name,
          email: values.email,
          birthDate: values.birthDate,
          cep: values.cep.replace(/\D/g, ''),
          street: values.street,
          number: undefined,
          complement: values.complement,
          neighborhood: values.neighborhood,
          city: values.city,
          password: values.password,
          consentimentoAceito: values.consentimentoAceito,
        });
        setCurrentUserId(result?.user?.id ?? null);
        navigation.replace('Onboarding');
      }
      return {
        ok: true,
      };
    } catch (error) {
      return {
        ok: false,
        reason: 'api',
        message: error.message,
      };
    } finally {
      setLoading(false);
    }
  }

  function validateRegisterStep(scope) {
    const validationErrors = validateAuthValues(mode, values, scope);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return {
        ok: false,
        reason: 'validation',
      };
    }

    setErrors({});
    return {
      ok: true,
    };
  }

  async function handleGoogleAuth() {
    setLoading(true);

    try {
      await authenticateWithGoogle();
    } catch (error) {
      return {
        ok: false,
        reason: 'api',
        message: error.message,
      };
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
    cepLookupLoading,
    setMode: handleModeChange,
    updateField,
    validateRegisterStep,
    handleSubmit,
    handleGoogleAuth,
  };
}
