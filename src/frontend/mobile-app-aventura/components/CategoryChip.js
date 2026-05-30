import React from 'react';
import { Pressable, Text, StyleSheet } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function CategoryChip({
  label,
  selected = false,
  onPress,
}) {
  return (
    <Pressable
      onPress={onPress}
      style={({ pressed }) => [
        styles.chip,
        selected && styles.selectedChip,
        pressed && styles.pressedChip,
      ]}
    >
      <Text style={[styles.label, selected && styles.selectedLabel]}>
        {label}
      </Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  chip: {
    minHeight: 40,
    minWidth: 78,
    paddingHorizontal: spacing.md,
    borderRadius: radius.lg,
    borderWidth: 1,
    borderColor: 'rgba(224, 212, 255, 0.9)',
    backgroundColor: 'rgba(255, 255, 255, 0.92)',
    alignItems: 'center',
    justifyContent: 'center',

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 3,
    },
    shadowOpacity: 0.04,
    shadowRadius: 8,
    elevation: 2,
  },

  selectedChip: {
    backgroundColor: colors.purpleSoft,
    borderColor: colors.purpleLight,

    shadowColor: colors.purplePrimary,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.12,
    shadowRadius: 10,
    elevation: 4,
  },

  pressedChip: {
    opacity: 0.88,
    transform: [{ scale: 0.98 }],
  },

  label: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayText,
  },

  selectedLabel: {
    color: colors.purplePrimary,
    fontWeight: typography.fontWeight.semiBold,
  },
});