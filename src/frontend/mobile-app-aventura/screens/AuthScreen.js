import React, { useState } from 'react';
import {
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
  View,
} from 'react-native';

import AuthForm from '../components/AuthForm';
import AuthHeader from '../components/AuthHeader';
import AuthModeSwitch from '../components/AuthModeSwitch';
import SystemStatus from '../components/SystemStatus';
import { useAuthViewModel } from '../viewmodels/useAuthViewModel';
import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';
import ScreenContainer from './ScreenContainer';

export default function AuthScreen({ navigation }) {
  const [systemStatus, setSystemStatus] = useState(null);

  const {
    mode,
    values,
    errors,
    content,
    loading,
    cepLookupLoading,
    setMode,
    updateField,
    validateRegisterStep,
    handleSubmit,
    handleGoogleAuth,
  } = useAuthViewModel(navigation);

  async function handleSubmitWithStatus() {
    const result = await handleSubmit();

    if (result?.reason === 'validation') {
      setSystemStatus({
        type: 'warning',
        message: 'Revise os campos destacados para continuar.',
      });
    } else if (result?.reason === 'api') {
      setSystemStatus({
        type: 'warning',
        message: result.message,
      });
    }
  }

  async function handleGoogleAuthWithStatus() {
    setSystemStatus({
      type: 'info',
      message: 'Iniciando autenticação com Google.',
    });

    const result = await handleGoogleAuth();
    if (result?.reason === 'api') {
      setSystemStatus({
        type: 'warning',
        message: result.message,
      });
    }
  }

  return (
    <ScreenContainer style={styles.screen}>
      <KeyboardAvoidingView
        style={styles.keyboardView}
        behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      >
        <ScrollView
          contentContainerStyle={styles.content}
          keyboardDismissMode="on-drag"
          keyboardShouldPersistTaps="handled"
          showsVerticalScrollIndicator={false}
        >
          <AuthHeader
            subtitle={content.subtitle}
          />

          <View style={styles.formArea}>
            <AuthForm
              mode={mode}
              values={values}
              errors={errors}
              submitLabel={content.submitLabel}
              loading={loading}
              cepLookupLoading={cepLookupLoading}
              onChangeField={updateField}
              onValidateStep={validateRegisterStep}
              onSubmit={handleSubmitWithStatus}
              onGoogleAuth={handleGoogleAuthWithStatus}
            />
          </View>

          <AuthModeSwitch mode={mode} onChange={setMode} />
        </ScrollView>
      </KeyboardAvoidingView>

      <SystemStatus
        message={systemStatus?.message}
        type={systemStatus?.type}
        onDismiss={() => setSystemStatus(null)}
      />
    </ScreenContainer>
  );
}

const styles = StyleSheet.create({
  screen: {
    backgroundColor: colors.white,
  },

  keyboardView: {
    flex: 1,
  },

  content: {
    flexGrow: 1,
    justifyContent: 'flex-start',
    gap: spacing.xxxl,
    paddingTop: spacing.xxxl,
    paddingBottom: spacing.xl,
    paddingHorizontal: spacing.lg,
  },

  formArea: {
    width: '100%',
    gap: spacing.lg,
  },
});
