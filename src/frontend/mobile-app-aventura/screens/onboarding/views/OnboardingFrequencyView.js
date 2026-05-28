import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

import SelectableOption from '../components/SelectableOption';
import OnboardingLayout from '../components/OnboardingLayout';
import { FREQUENCIES } from '../../../models/onboardingData';
import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing } from '../../../constants/spacing';

export default function OnboardingFrequencyView({
  step,
  totalSteps,
  selectedFrequency,
  onSelectFrequency,
  onContinue,
  onSkip,
  onBack,
  animatedStyle,
}) {
  return (
    <OnboardingLayout
      step={step}
      totalSteps={totalSteps}
      onSkip={onSkip}
      onBack={onBack}
      onContinue={onContinue}
      canContinue={selectedFrequency !== null}
      animatedStyle={animatedStyle}
    >
      <Text style={styles.title}>Com que frequência você vai ao teatro?</Text>

      <View style={styles.list}>
        {FREQUENCIES.map((frequency) => (
          <SelectableOption
            key={frequency.id}
            label={frequency.label}
            subtitle={frequency.subtitle}
            selected={selectedFrequency === frequency.id}
            onPress={() => onSelectFrequency(frequency.id)}
          />
        ))}
      </View>
    </OnboardingLayout>
  );
}

const styles = StyleSheet.create({
  title: {
    fontSize: typography.size.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    marginBottom: spacing.xl,
    lineHeight: typography.lineHeight.xl,
  },

  list: {
    gap: spacing.sm,
  },
});
