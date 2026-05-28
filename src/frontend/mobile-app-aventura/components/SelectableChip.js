import React from 'react';

import {
  TouchableOpacity,
  Text,
  StyleSheet,
} from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function SelectableChip({
  label,
  selected,
  onPress,
}) {
  return (
    <TouchableOpacity
      style={[
        styles.container,
        selected && styles.selectedContainer,
      ]}
      onPress={onPress}
      activeOpacity={0.8}
    >
      <Text
        style={[
          styles.label,
          selected && styles.selectedLabel,
        ]}
      >
        {selected ? '✓ ' : ''}
        {label}
      </Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    minHeight: 52,

    borderWidth: 1,
    borderColor: colors.grayLight,

    borderRadius: radius.md,

    alignItems: 'center',
    justifyContent: 'center',

    paddingHorizontal: spacing.lg,

    backgroundColor: colors.white,
  },

  selectedContainer: {
    borderColor: colors.purplePrimary,
    backgroundColor: '#F3ECFF',
  },

  label: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    color: colors.grayDark,
  },

  selectedLabel: {
    color: colors.grayDark,
    fontWeight: typography.fontWeight.medium,
  },
});
