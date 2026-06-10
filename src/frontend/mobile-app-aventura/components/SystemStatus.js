import React, { useEffect } from 'react';

import {
  View,
  Text,
  StyleSheet,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

const statusConfig = {
  success: {
    icon: 'checkmark-circle',
    color: colors.purplePrimary,
  },
  info: {
    icon: 'information-circle',
    color: colors.grayDark,
  },
  warning: {
    icon: 'alert-circle',
    color: colors.peachBurnt,
  },
};

export default function SystemStatus({
  message,
  type = 'success',
  onDismiss,
  duration = 2600,
}) {
  useEffect(() => {
    if (!message) return undefined;

    const timer = setTimeout(() => {
      onDismiss?.();
    }, duration);

    return () => clearTimeout(timer);
  }, [duration, message, onDismiss]);

  if (!message) {
    return null;
  }

  const currentStatus = statusConfig[type] || statusConfig.success;

  return (
    <View style={styles.container} pointerEvents="none">
      <View style={styles.toast}>
        <Ionicons
          name={currentStatus.icon}
          size={20}
          color={currentStatus.color}
        />

        <Text style={styles.message}>
          {message}
        </Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    position: 'absolute',
    left: spacing.md,
    right: spacing.md,
    bottom: 112,
    zIndex: 50,
    alignItems: 'center',
  },

  toast: {
    maxWidth: 340,
    minHeight: 48,
    borderRadius: radius.md,
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 5,
    },
    shadowOpacity: 0.08,
    shadowRadius: 14,
    elevation: 6,
  },

  message: {
    flex: 1,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },
});
