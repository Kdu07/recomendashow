import React from 'react';
import { TouchableOpacity, Text, StyleSheet } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function CategoryChip({
  label,
  selected = false,
  onPress,
}) {
  return (
    <TouchableOpacity
      style={[styles.chip, selected && styles.selectedChip]}
      onPress={onPress}
      activeOpacity={0.8}
    >
      <Text style={[styles.label, selected && styles.selectedLabel]}>
        {selected ? `✓ ${label}` : label}
      </Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  chip: {
    minHeight: 42,
    borderRadius: radius.md,
    borderWidth: 1,
    borderColor: colors.grayLight,
    backgroundColor: colors.white,
    alignItems: 'center',
    justifyContent: 'center',
    paddingHorizontal: spacing.lg,
  },

  selectedChip: {
    borderColor: colors.purplePrimary,
    backgroundColor: colors.purpleSoft,
  },

  label: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },

  selectedLabel: {
    color: colors.purplePrimary,
  },
});