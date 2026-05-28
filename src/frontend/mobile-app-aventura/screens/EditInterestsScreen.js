import React from 'react';

import {
  View,
  Text,
  StyleSheet,
  ScrollView,
} from 'react-native';

import AppButton from '../components/AppButton';
import BottomTabBar from '../components/BottomTabBar';
import InfoCard from '../components/InfoCard';
import SelectableChip from '../components/SelectableChip';
import ScreenHeader from '../components/ScreenHeader';

import useEditInterestsViewModel from '../viewmodels/useEditInterestsViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing } from '../constants/spacing';

export default function EditInterestsScreen({
  navigation,
}) {
  const {
    interests,
    cities,

    selectedInterestsCount,

    handleToggleInterest,
    handleSelectCity,
    handleSaveChanges,
  } = useEditInterestsViewModel();

  return (
    <View style={styles.page}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        <ScreenHeader
          title="EDITAR INTERESSES"
          onBackPress={() =>
            navigation.navigate('Profile')
          }
        />

        <InfoCard
          title="Ajuste suas recomendações"
          description="Suas escolhas ajudam a personalizar o conteúdo que você vê no Aventura"
        />

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>
            SELECIONE SEUS INTERESSES
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
            SELECIONE SUA CIDADE
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
            title="✓ SALVAR ALTERAÇÕES"
            onPress={handleSaveChanges}
          />
        </View>

        <View style={styles.tipContainer}>
          <Text style={styles.tipIcon}>
            💡
          </Text>

          <View style={styles.tipContent}>
            <Text style={styles.tipTitle}>
              Dica
            </Text>

            <Text style={styles.tipDescription}>
              Você pode alterar seus interesses a qualquer momento.
              Quanto mais específico, melhores serão suas recomendações!
            </Text>
          </View>
        </View>
      </ScrollView>

      <BottomTabBar
        activeTab="profile"
        onTabPress={(tab) => {
          if (tab === 'explore') {
            navigation.navigate('Home');
            return;
          }

          if (tab === 'profile') {
            navigation.navigate('Profile');
            return;
          }
        }}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  page: {
    flex: 1,
    backgroundColor: '#FAFAFA',
  },

  scrollContent: {
    padding: spacing.lg,
    paddingBottom: 140,
    gap: spacing.xl,
  },

  header: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  section: {
    gap: spacing.lg,
  },

  sectionTitle: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.bold,
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
  },

  cityList: {
    gap: spacing.md,
  },

  buttonContainer: {
    marginTop: spacing.lg,
  },

  tipContainer: {
    borderTopWidth: 1,
    borderTopColor: colors.grayLight,

    paddingTop: spacing.xl,

    flexDirection: 'row',
    gap: spacing.md,
  },

  tipIcon: {
    fontSize: 22,
  },

  tipContent: {
    flex: 1,
    gap: spacing.xs,
  },

  tipTitle: {
    fontSize: typography.size.lg,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  tipDescription: {
    fontSize: typography.size.md,
    color: colors.grayText,
    lineHeight: 26,
  },
});