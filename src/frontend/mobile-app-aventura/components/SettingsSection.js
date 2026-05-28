import React from 'react';

import {
  View,
  Text,
  StyleSheet,
} from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function SettingsSection({
  title,
  children,
}) {
  return (
    <View style={styles.wrapper}>
      <Text style={styles.title}>
        {title}
      </Text>

      <View style={styles.card}>
        {children}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  wrapper: {
    gap: spacing.md,
  },

  title: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayText,
  },

  card: {
    backgroundColor: colors.white,

    borderWidth: 1,
    borderColor: colors.grayLight,

    borderRadius: radius.lg,

    padding: spacing.lg,
  },
});