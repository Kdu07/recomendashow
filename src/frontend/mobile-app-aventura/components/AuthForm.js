import React from 'react';
import { StyleSheet, View } from 'react-native';

import { spacing } from '../constants/spacing';
import AppButton from './AppButton';
import AppInput from './AppInput';
import AuthDivider from './AuthDivider';

export default function AuthForm({
  mode,
  values,
  errors = {},
  submitLabel,
  loading = false,
  onChangeField,
  onSubmit,
  onGoogleAuth,
}) {
  const isLogin = mode === 'login';

  const fields = [
    !isLogin && {
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
      name: 'password',
      label: 'Senha',
      placeholder: isLogin ? 'Digite sua senha' : 'Mínimo 8 caracteres',
      secureTextEntry: true,
      textContentType: isLogin ? 'password' : 'newPassword',
      autoComplete: isLogin ? 'password' : 'new-password',
      autoCapitalize: 'none',
      autoCorrect: false,
      returnKeyType: isLogin ? 'done' : 'next',
    },
    !isLogin && {
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
  ].filter(Boolean);

  return (
    <View style={styles.container}>
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
          />
        ))}
      </View>

      <AppButton title={submitLabel} loading={loading} onPress={onSubmit} />

      <AuthDivider />

      <AppButton
        title="Google"
        variant="secondary"
        disabled={loading}
        showGoogleIcon
        onPress={onGoogleAuth}
      />
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
});
