import React from 'react';
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
import { useAuthViewModel } from '../viewmodels/useAuthViewModel';
import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';
import ScreenContainer from './ScreenContainer';

export default function AuthScreen() {
  const {
    mode,
    values,
    errors,
    content,
    loading,
    setMode,
    updateField,
    handleSubmit,
    handleGoogleAuth,
  } = useAuthViewModel();

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
            mode={mode}
          />

          <View style={styles.formArea}>
            <AuthForm
              mode={mode}
              values={values}
              errors={errors}
              submitLabel={content.submitLabel}
              loading={loading}
              onChangeField={updateField}
              onSubmit={handleSubmit}
              onGoogleAuth={handleGoogleAuth}
            />
          </View>

          <AuthModeSwitch mode={mode} onChange={setMode} />
        </ScrollView>
      </KeyboardAvoidingView>
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
