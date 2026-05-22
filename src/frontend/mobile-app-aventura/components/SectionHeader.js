import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing } from '../constants/spacing';

export default function SectionHeader({ title, subtitle }) {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>{title}</Text>

      {subtitle && (
        <Text style={styles.subtitle}>
          {subtitle}
        </Text>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.xs,
    marginTop: spacing.lg,
  },

  title: {
    fontSize: typography.size.lg,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  subtitle: {
    fontSize: typography.size.sm,
    color: colors.grayText,
  },
});