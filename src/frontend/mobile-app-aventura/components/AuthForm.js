import React, { useEffect, useMemo, useState } from 'react';
import { StyleSheet, Text, TouchableOpacity, View } from 'react-native';
import { Ionicons } from '@expo/vector-icons';

import { colors } from '../constants/colors';
import { radius, spacing } from '../constants/spacing';
import { typography } from '../constants/typography';

import AppButton from './AppButton';
import AppInput from './AppInput';
import AuthDivider from './AuthDivider';

export default function AuthForm({
  mode,
  values,
  errors = {},
  submitLabel,
  loading = false,
  cepLookupLoading = false,
  onChangeField,
  onValidateStep,
  onSubmit,
  onGoogleAuth,
}) {
  const isLogin = mode === 'login';
  const [registerStep, setRegisterStep] = useState(1);

  useEffect(() => {
    setRegisterStep(1);
  }, [mode]);

  const accountFields = useMemo(() => ([
    {
      name: 'name',
      label: 'Nome completo',
      placeholder: 'Digite seu nome',
      textContentType: 'name',
      autoComplete: 'name',
      autoCapitalize: 'words',
      returnKeyType: 'next',
    },
    {
      name: 'email',
      label: 'E-mail',
      placeholder: 'voce@email.com',
      keyboardType: 'email-address',
      textContentType: 'emailAddress',
      autoComplete: 'email',
      autoCapitalize: 'none',
      autoCorrect: false,
      returnKeyType: 'next',
    },
    {
      name: 'birthDate',
      label: 'Data de nascimento',
      placeholder: 'DD/MM/AAAA',
      keyboardType: 'numbers-and-punctuation',
      textContentType: 'birthdate',
      autoComplete: 'birthdate-full',
      autoCapitalize: 'none',
      autoCorrect: false,
      returnKeyType: 'next',
    },
    {
      name: 'password',
      label: 'Senha',
      placeholder: 'Mínimo 6 caracteres',
      secureTextEntry: true,
      textContentType: 'newPassword',
      autoComplete: 'new-password',
      autoCapitalize: 'none',
      autoCorrect: false,
      returnKeyType: 'next',
    },
    {
      name: 'confirmPassword',
      label: 'Confirmar senha',
      placeholder: 'Digite a senha novamente',
      secureTextEntry: true,
      textContentType: 'newPassword',
      autoComplete: 'new-password',
      autoCapitalize: 'none',
      autoCorrect: false,
      returnKeyType: 'done',
    },
  ]), []);

  const addressFields = useMemo(() => ([
    {
      name: 'cep',
      label: 'CEP',
      placeholder: '00000-000',
      keyboardType: 'number-pad',
      textContentType: 'postalCode',
      autoComplete: 'postal-code',
      autoCapitalize: 'none',
      autoCorrect: false,
      returnKeyType: 'next',
      helperText: cepLookupLoading ? 'Buscando endereço pelo CEP...' : undefined,
    },
    {
      name: 'street',
      label: 'Rua',
      placeholder: 'Preenchida pelo CEP',
      textContentType: 'streetAddressLine1',
      autoComplete: 'street-address',
      autoCapitalize: 'words',
      autoCorrect: false,
      returnKeyType: 'next',
      editable: false,
    },
    {
      name: 'complement',
      label: 'Complemento',
      placeholder: 'Apartamento, bloco ou referência',
      textContentType: 'streetAddressLine2',
      autoComplete: 'off',
      autoCapitalize: 'sentences',
      autoCorrect: false,
      returnKeyType: 'next',
    },
    {
      name: 'neighborhood',
      label: 'Bairro',
      placeholder: 'Preenchido pelo CEP',
      textContentType: 'addressCity',
      autoComplete: 'off',
      autoCapitalize: 'words',
      autoCorrect: false,
      returnKeyType: 'next',
      editable: false,
    },
    {
      name: 'city',
      label: 'Cidade',
      placeholder: 'Preenchida pelo CEP',
      textContentType: 'addressCity',
      autoComplete: 'address-line2',
      autoCapitalize: 'words',
      autoCorrect: false,
      returnKeyType: 'done',
      editable: false,
    },
  ]), [cepLookupLoading]);

  const loginFields = useMemo(() => ([
    {
      name: 'email',
      label: 'E-mail',
      placeholder: 'voce@email.com',
      keyboardType: 'email-address',
      textContentType: 'emailAddress',
      autoComplete: 'email',
      autoCapitalize: 'none',
      autoCorrect: false,
      returnKeyType: 'next',
    },
    {
      name: 'password',
      label: 'Senha',
      placeholder: 'Digite sua senha',
      secureTextEntry: true,
      textContentType: 'password',
      autoComplete: 'password',
      autoCapitalize: 'none',
      autoCorrect: false,
      returnKeyType: 'done',
    },
  ]), []);

  const fields = isLogin
    ? loginFields
    : registerStep === 1
      ? accountFields
      : addressFields;

  async function handleContinue() {
    const result = onValidateStep?.('account');

    if (result?.ok) {
      setRegisterStep(2);
    }
  }

  return (
    <View style={styles.container}>
      {!isLogin && (
        <View style={styles.stepTabs}>
          <View style={[styles.stepTab, registerStep === 1 && styles.activeStepTab]}>
            <Text style={[styles.stepText, registerStep === 1 && styles.activeStepText]}>
              Dados
            </Text>
          </View>
          <View style={[styles.stepTab, registerStep === 2 && styles.activeStepTab]}>
            <Text style={[styles.stepText, registerStep === 2 && styles.activeStepText]}>
              Endereço
            </Text>
          </View>
        </View>
      )}

      <View style={styles.fields}>
        {fields.map((field) => (
          <AppInput
            key={field.name}
            label={field.label}
            placeholder={field.placeholder}
            value={values[field.name]}
            error={errors[field.name]}
            onChangeText={(value) => onChangeField(field.name, value)}
            secureTextEntry={field.secureTextEntry}
            keyboardType={field.keyboardType}
            autoComplete={field.autoComplete}
            autoCapitalize={field.autoCapitalize}
            autoCorrect={field.autoCorrect}
            textContentType={field.textContentType}
            returnKeyType={field.returnKeyType}
            editable={field.editable !== false}
            helperText={field.helperText}
          />
        ))}
      </View>

      {!isLogin && registerStep === 2 && (
        <TouchableOpacity
          style={styles.consentRow}
          onPress={() => onChangeField('consentimentoAceito', !values.consentimentoAceito)}
          activeOpacity={0.7}
          accessibilityRole="checkbox"
          accessibilityState={{ checked: values.consentimentoAceito }}
        >
          <Ionicons
            name={values.consentimentoAceito ? 'checkbox' : 'square-outline'}
            size={22}
            color={values.consentimentoAceito ? colors.purplePrimary : colors.grayText}
          />
          <Text style={[styles.consentLabel, errors.consentimentoAceito && styles.consentLabelError]}>
            Li e aceito a Política de Privacidade
          </Text>
        </TouchableOpacity>
      )}

      {isLogin ? (
        <AppButton title={submitLabel} loading={loading} onPress={onSubmit} />
      ) : registerStep === 2 ? (
        <View style={styles.actions}>
          <AppButton
            title="Voltar"
            variant="secondary"
            disabled={loading}
            style={styles.actionButton}
            onPress={() => setRegisterStep(1)}
          />
          <AppButton
            title={submitLabel}
            loading={loading}
            style={styles.actionButton}
            onPress={onSubmit}
          />
        </View>
      ) : (
        <AppButton
          title="Continuar"
          disabled={loading}
          onPress={handleContinue}
        />
      )}

      {(isLogin || registerStep === 1) && (
        <>
          <AuthDivider />

          <AppButton
            title="Google"
            variant="secondary"
            disabled={loading}
            showGoogleIcon
            onPress={onGoogleAuth}
          />
        </>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.lg,
  },

  fields: {
    gap: spacing.md,
  },

  stepTabs: {
    flexDirection: 'row',
    gap: spacing.sm,
    padding: spacing.xs,
    borderRadius: radius.sm,
    backgroundColor: colors.whiteSoft,
  },

  stepTab: {
    flex: 1,
    height: 36,
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: radius.sm,
  },

  activeStepTab: {
    backgroundColor: colors.white,
  },

  stepText: {
    fontSize: typography.size.xs,
    lineHeight: typography.lineHeight.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayText,
  },

  activeStepText: {
    color: colors.purplePrimary,
  },

  actions: {
    flexDirection: 'row',
    gap: spacing.sm,
  },

  actionButton: {
    flex: 1,
  },

  consentRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
  },

  consentLabel: {
    flex: 1,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },

  consentLabelError: {
    color: colors.peachBurnt,
  },
});
