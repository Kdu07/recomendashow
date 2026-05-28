import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';
import { Lightbulb } from 'lucide-react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing, radius } from '../constants/spacing';

export default function EmptyState({
  title,
  description,
  actionLabel,
  onActionPress,
}) {
  return (
    <View style={styles.container}>
      <View style={styles.iconOuter}>
        <View style={styles.iconInner}>
          <Lightbulb
            size={34}
            strokeWidth={2.4}
            color={colors.peachBurnt}
          />
        </View>
      </View>

      <Text style={styles.title}>{title}</Text>

      <Text style={styles.description}>{description}</Text>

      {actionLabel && (
        <TouchableOpacity
          style={styles.actionButton}
          onPress={onActionPress}
          activeOpacity={0.78}
        >
          <Text style={styles.actionText}>{actionLabel}</Text>
        </TouchableOpacity>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: spacing.xxl,
    paddingHorizontal: spacing.lg,
    backgroundColor: colors.white,
  },

  iconOuter: {
    width: 82,
    height: 82,
    borderRadius: radius.pill,
    backgroundColor: colors.whiteSoft,
    borderWidth: 1,
    borderColor: colors.grayLight,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: spacing.md,

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.05,
    shadowRadius: 10,
    elevation: 2,
  },

  iconInner: {
    width: 56,
    height: 56,
    borderRadius: radius.pill,
    backgroundColor: colors.white,
    alignItems: 'center',
    justifyContent: 'center',
  },

  title: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    textAlign: 'center',
    marginBottom: spacing.sm,
  },

  description: {
    maxWidth: 285,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
    textAlign: 'center',
    marginBottom: spacing.md,
  },

  actionButton: {
    paddingVertical: spacing.sm,
    paddingHorizontal: spacing.md,
    borderRadius: radius.md,
    backgroundColor: colors.purpleSoft,
  },

  actionText: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.purplePrimary,
  },
});