import React, { useState } from 'react';

import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  SafeAreaView,
} from 'react-native';

import AppButton from '../components/AppButton';
import InfoCard from '../components/InfoCard';
import SelectableChip from '../components/SelectableChip';
import ScreenHeader from '../components/ScreenHeader';
import SystemStatus from '../components/SystemStatus';

import useEditInterestsViewModel from '../viewmodels/useEditInterestsViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function EditInterestsScreen({
  navigation,
}) {
  const [systemStatus, setSystemStatus] = useState(null);

  const {
    interests,
    cities,

    selectedInterestsCount,

    handleToggleInterest,
    handleSelectCity,
    handleSaveChanges,
  } = useEditInterestsViewModel();

  async function handleSaveWithStatus() {
    const result = await handleSaveChanges();

    setSystemStatus({
      type: result.savedOnServer ? 'success' : 'warning',
      message: `${result.selectedInterestsCount} interesse(s) salvos para ${result.selectedCity || 'sua cidade'}.`,
    });
  }

  return (
    <View style={styles.page}>
      <SafeAreaView style={styles.contentSafeArea}>
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={styles.scrollContent}
        >
          <ScreenHeader
            title="Preferências"
            onBackPress={() =>
              navigation.navigate('MainTabs', { screen: 'Profile' })
            }
          />

          <InfoCard
            title="Ajuste suas recomendações"
            description="Suas escolhas ajudam a personalizar o conteúdo que você vê no Aventura."
          />

          <View style={styles.section}>
            <Text style={styles.sectionTitle}>
              Interesses
            </Text>

            <View style={styles.interestsGrid}>
              {interests.map((interest) => (
                <View
                  key={interest.id}
                  style={styles.interestItem}
                >
                  <SelectableChip
                    label={interest.label}
                    selected={interest.selected}
                    onPress={() =>
                      handleToggleInterest(
                        interest.id
                      )
                    }
                  />
                </View>
              ))}
            </View>

            <Text style={styles.counter}>
              {selectedInterestsCount} interesses selecionados
            </Text>
          </View>

          <View style={styles.section}>
            <Text style={styles.sectionTitle}>
              Cidade
            </Text>

            <View style={styles.cityList}>
              {cities.map((city) => (
                <SelectableChip
                  key={city.id}
                  label={city.label}
                  selected={city.selected}
                  onPress={() =>
                    handleSelectCity(city.id)
                  }
                />
              ))}
            </View>
          </View>

          <View style={styles.buttonContainer}>
            <AppButton
              title="Salvar alterações"
              onPress={handleSaveWithStatus}
            />
          </View>

          <View style={styles.tipContainer}>
            <Text style={styles.tipTitle}>
              Dica
            </Text>

            <Text style={styles.tipDescription}>
              Você pode alterar seus interesses a qualquer momento. Quanto mais específico, melhores serão suas recomendações.
            </Text>
          </View>
        </ScrollView>
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

  scrollContent: {
    paddingHorizontal: spacing.md,
    paddingTop: spacing.md,
    paddingBottom: spacing.xxxl,
    gap: spacing.lg,
  },

  section: {
    gap: spacing.md,
  },

  sectionTitle: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  interestsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.md,
  },

  interestItem: {
    width: '47%',
  },

  counter: {
    textAlign: 'center',
    color: colors.grayText,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
  },

  cityList: {
    gap: spacing.md,
  },

  buttonContainer: {
    marginTop: spacing.sm,
  },

  tipContainer: {
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,

    padding: spacing.lg,
    gap: spacing.xs,
  },

  tipTitle: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  tipDescription: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },
});
