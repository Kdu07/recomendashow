import React, { useState } from 'react';
import {
  KeyboardAvoidingView,
  Platform,
  SafeAreaView,
  ScrollView,
  StyleSheet,
  Text,
  View,
} from 'react-native';

import AppButton from '../components/AppButton';
import AppInput from '../components/AppInput';
import ScreenHeader from '../components/ScreenHeader';
import SystemStatus from '../components/SystemStatus';
import useEditProfileViewModel from '../viewmodels/useEditProfileViewModel';
import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';
import { typography } from '../constants/typography';

export default function EditProfileScreen({ navigation }) {
  const [systemStatus, setSystemStatus] = useState(null);
  const {
    values,
    errors,
    saving,
    updateField,
    saveProfile,
  } = useEditProfileViewModel();

  async function handleSave() {
    const result = await saveProfile();

    if (!result.ok) {
      setSystemStatus({
        type: 'warning',
        message: 'Revise os campos destacados para salvar.',
      });
      return;
    }

    setSystemStatus({
      type: 'success',
      message: 'Perfil atualizado com sucesso.',
    });

    setTimeout(() => {
      navigation.navigate('MainTabs', { screen: 'Profile' });
    }, 700);
  }

  return (
    <View style={styles.page}>
      <SafeAreaView style={styles.contentSafeArea}>
        <KeyboardAvoidingView
          style={styles.keyboardView}
          behavior={Platform.OS === 'ios' ? 'padding' : undefined}
        >
          <ScrollView
            contentContainerStyle={styles.scrollContent}
            keyboardDismissMode="on-drag"
            keyboardShouldPersistTaps="handled"
            showsVerticalScrollIndicator={false}
          >
          <ScreenHeader
            title="Editar perfil"
            onBackPress={() => navigation.navigate('MainTabs', { screen: 'Profile' })}
          />

          <View style={styles.form}>
            <View style={styles.section}>
              <Text style={styles.h2}>
                Dados pessoais
              </Text>

              <AppInput
                label="Nome completo"
                placeholder="Digite seu nome"
                value={values.name}
                error={errors.name}
                autoCapitalize="words"
                autoComplete="name"
                textContentType="name"
                returnKeyType="next"
                onChangeText={(value) => updateField('name', value)}
              />

              <AppInput
                label="E-mail"
                placeholder="voce@email.com"
                value={values.email}
                error={errors.email}
                keyboardType="email-address"
                autoCapitalize="none"
                autoCorrect={false}
                autoComplete="email"
                textContentType="emailAddress"
                returnKeyType="next"
                onChangeText={(value) => updateField('email', value)}
              />

              <AppInput
                label="Data de nascimento"
                placeholder="DD/MM/AAAA"
                value={values.birthDate}
                error={errors.birthDate}
                keyboardType="numbers-and-punctuation"
                autoCapitalize="none"
                autoCorrect={false}
                autoComplete="birthdate-full"
                textContentType="birthdate"
                returnKeyType="next"
                onChangeText={(value) => updateField('birthDate', value)}
              />
            </View>

            <View style={styles.section}>
              <Text style={styles.h2}>
                Endereço
              </Text>

              <View style={styles.inputRow}>
                <View style={styles.compactInput}>
                  <AppInput
                    label="CEP"
                    placeholder="00000-000"
                    value={values.cep}
                    error={errors.cep}
                    keyboardType="number-pad"
                    autoCapitalize="none"
                    autoCorrect={false}
                    autoComplete="postal-code"
                    textContentType="postalCode"
                    returnKeyType="next"
                    onChangeText={(value) => updateField('cep', value)}
                  />
                </View>

                <View style={styles.compactInput}>
                  <AppInput
                    label="Cidade"
                    placeholder="Cidade"
                    value={values.city}
                    error={errors.city}
                    autoCapitalize="words"
                    autoCorrect={false}
                    autoComplete="address-line2"
                    textContentType="addressCity"
                    returnKeyType="next"
                    onChangeText={(value) => updateField('city', value)}
                  />
                </View>
              </View>

              <View style={styles.inputRow}>
                <View style={styles.compactInput}>
                  <AppInput
                    label="Bairro"
                    placeholder="Bairro"
                    value={values.neighborhood}
                    error={errors.neighborhood}
                    autoCapitalize="words"
                    autoCorrect={false}
                    autoComplete="off"
                    textContentType="addressCity"
                    returnKeyType="next"
                    onChangeText={(value) => updateField('neighborhood', value)}
                  />
                </View>

                <View style={styles.compactInput}>
                  <AppInput
                    label="Rua"
                    placeholder="Rua"
                    value={values.street}
                    error={errors.street}
                    autoCapitalize="words"
                    autoCorrect={false}
                    autoComplete="street-address"
                    textContentType="streetAddressLine1"
                    returnKeyType="next"
                    onChangeText={(value) => updateField('street', value)}
                  />
                </View>
              </View>

              <View style={styles.inputRow}>
                <View style={styles.compactInput}>
                  <AppInput
                    label="Número"
                    placeholder="Nº"
                    value={values.number}
                    error={errors.number}
                    keyboardType="number-pad"
                    autoCapitalize="none"
                    autoCorrect={false}
                    autoComplete="off"
                    textContentType="none"
                    returnKeyType="next"
                    onChangeText={(value) => updateField('number', value)}
                  />
                </View>

                <View style={styles.compactInput}>
                  <AppInput
                    label="Complemento"
                    placeholder="Complemento"
                    value={values.complement}
                    error={errors.complement}
                    autoCapitalize="sentences"
                    autoCorrect={false}
                    autoComplete="off"
                    textContentType="streetAddressLine2"
                    returnKeyType="done"
                    onChangeText={(value) => updateField('complement', value)}
                  />
                </View>
              </View>
            </View>
          </View>

          <AppButton
            title="Salvar alterações"
            loading={saving}
            onPress={handleSave}
          />
          </ScrollView>
        </KeyboardAvoidingView>
      </SafeAreaView>

      <SystemStatus
        message={systemStatus?.message}
        type={systemStatus?.type}
        onDismiss={() => setSystemStatus(null)}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  page: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  contentSafeArea: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  keyboardView: {
    flex: 1,
  },

  scrollContent: {
    flexGrow: 1,
    paddingHorizontal: spacing.md,
    paddingTop: spacing.md,
    paddingBottom: spacing.xxxl,
    gap: spacing.lg,
  },

  form: {
    gap: spacing.lg,
  },

  section: {
    gap: spacing.md,
  },

  h2: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  inputRow: {
    flexDirection: 'row',
    gap: spacing.md,
    alignItems: 'flex-start',
  },

  compactInput: {
    flex: 1,
    minWidth: 0,
  },

});
