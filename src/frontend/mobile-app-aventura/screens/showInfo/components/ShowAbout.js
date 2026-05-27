import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing } from '../../../constants/spacing';

export default function ShowAbout({ about }) {
  return (
    <View style={styles.container}>
      <View style={styles.divider} />

      <Text style={styles.sectionLabel}>{about.sectionLabel}</Text>

      <Text style={styles.description}>{about.description}</Text>

      <Text style={styles.duration}>{about.duration}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.lg,
    gap: spacing.md,
  },

  divider: {
    height: 1,
    backgroundColor: colors.grayLight,
    marginBottom: spacing.sm,
  },

  sectionLabel: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    letterSpacing: 0.5,
  },

  description: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayDark,
    lineHeight: typography.lineHeight.sm,
  },

  duration: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },
});
