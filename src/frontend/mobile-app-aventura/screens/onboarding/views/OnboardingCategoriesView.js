import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

import CategoryChip from '../../../components/CategoryChip';
import OnboardingLayout from '../components/OnboardingLayout';
import { CATEGORIES } from '../../../models/onboardingData';
import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing } from '../../../constants/spacing';

export default function OnboardingCategoriesView({
  step,
  totalSteps,
  selectedCategories,
  onToggleCategory,
  onContinue,
  onSkip,
  animatedStyle,
}) {
  return (
    <OnboardingLayout
      step={step}
      totalSteps={totalSteps}
      onSkip={onSkip}
      onContinue={onContinue}
      canContinue={selectedCategories.length > 0}
      animatedStyle={animatedStyle}
    >
      <Text style={styles.title}>O que te inspira?</Text>
      <Text style={styles.subtitle}>
        Selecione as categorias que mais te interessam para personalizar sua
        experiência conosco.
      </Text>

      <View style={styles.grid}>
        {CATEGORIES.map((category) => (
          <View key={category.id} style={styles.chipWrapper}>
            <CategoryChip
              label={category.label}
              selected={selectedCategories.includes(category.id)}
              onPress={() => onToggleCategory(category.id)}
            />
          </View>
        ))}
      </View>

      {selectedCategories.length > 0 && (
        <Text style={styles.counter}>
          {selectedCategories.length} de {CATEGORIES.length} categorias selecionadas
        </Text>
      )}
    </OnboardingLayout>
  );
}

const styles = StyleSheet.create({
  title: {
    fontSize: typography.size.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    marginBottom: spacing.sm,
  },

  subtitle: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
    marginBottom: spacing.xl,
    lineHeight: typography.lineHeight.sm,
  },

  grid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.sm,
  },

  chipWrapper: {
    width: '48%',
  },

  counter: {
    marginTop: spacing.lg,
    fontSize: typography.size.sm,
    color: colors.grayText,
    textAlign: 'center',
  },
});
