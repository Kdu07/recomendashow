import React from 'react';
import { TouchableOpacity, View, Text, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { radius, spacing } from '../../../constants/spacing';

export default function SelectableOption({ label, subtitle, selected = false, onPress }) {
  return (
    <TouchableOpacity
      style={[styles.option, selected && styles.optionSelected]}
      onPress={onPress}
      activeOpacity={0.8}
    >
      <View style={styles.textGroup}>
        <Text style={[styles.label, selected && styles.labelSelected]}>
          {selected ? `✓  ${label}` : label}
        </Text>
        {subtitle && (
          <Text style={[styles.subtitle, selected && styles.subtitleSelected]}>
            {subtitle}
          </Text>
        )}
      </View>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  option: {
    width: '100%',
    minHeight: 56,
    borderRadius: radius.lg,
    borderWidth: 1,
    borderColor: colors.grayLight,
    backgroundColor: colors.white,
    justifyContent: 'center',
    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.md,
  },

  optionSelected: {
    borderColor: colors.purplePrimary,
    backgroundColor: colors.purpleSoft,
  },

  textGroup: {
    gap: 2,
  },

  label: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },

  labelSelected: {
    color: colors.purplePrimary,
  },

  subtitle: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },

  subtitleSelected: {
    color: colors.purplePrimary,
  },
});
