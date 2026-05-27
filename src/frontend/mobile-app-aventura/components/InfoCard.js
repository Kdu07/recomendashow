import React from 'react';

import {
  View,
  Text,
  StyleSheet,
} from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function InfoCard({
  title,
  description,
}) {
  return (
    <View style={styles.container}>
      <Text style={styles.icon}>
        ⓘ
      </Text>

      <View style={styles.content}>
        <Text style={styles.title}>
          {title}
        </Text>

        <Text style={styles.description}>
          {description}
        </Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    borderWidth: 1,
    borderColor: '#FFB199',

    borderRadius: radius.lg,

    padding: spacing.lg,

    flexDirection: 'row',
    gap: spacing.md,
  },

  icon: {
    fontSize: 22,
    color: colors.peachBurnt,
  },

  content: {
    flex: 1,
    gap: spacing.xs,
  },

  title: {
    fontSize: typography.size.lg,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  description: {
    fontSize: typography.size.md,
    color: colors.grayText,
    lineHeight: 24,
  },
});