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
      <Text style={styles.title}>
        {title}
      </Text>

      <Text style={styles.description}>
        {description}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    backgroundColor: colors.white,

    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,

    padding: spacing.lg,
    gap: spacing.xs,

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.04,
    shadowRadius: 10,
    elevation: 2,
  },

  title: {
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  description: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
  },
});
