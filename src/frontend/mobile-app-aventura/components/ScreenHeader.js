import React from 'react';

import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
} from 'react-native';

import { Ionicons } from '@expo/vector-icons';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function ScreenHeader({
  title,
  onBackPress,
  rightComponent = null,
}) {
  return (
    <View style={styles.container}>
      <View style={styles.leftContent}>
        <TouchableOpacity
          style={styles.backButton}
          onPress={onBackPress}
          activeOpacity={0.7}
        >
          <Ionicons
            name="chevron-back"
            size={22}
            color={colors.grayDark}
          />
        </TouchableOpacity>

        <Text style={styles.title}>
          {title}
        </Text>
      </View>

      {rightComponent}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    minHeight: 48,

    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',

    marginBottom: spacing.sm,
  },

  leftContent: {
    flexDirection: 'row',
    alignItems: 'center',

    gap: spacing.md,
  },

  backButton: {
    width: 40,
    height: 40,

    borderRadius: radius.pill,

    backgroundColor: colors.white,

    borderWidth: 1,
    borderColor: colors.grayLight,

    alignItems: 'center',
    justifyContent: 'center',

    shadowColor: '#000',
    shadowOpacity: 0.04,
    shadowRadius: 6,
    shadowOffset: {
      width: 0,
      height: 2,
    },

    elevation: 2,
  },

  title: {
    fontSize: typography.size.lg,
    lineHeight: typography.lineHeight.lg,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },
});
