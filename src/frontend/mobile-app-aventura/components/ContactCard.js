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

export default function ContactCard({
  icon,
  value,
  link = false,
  onPress,
}) {
  const Container = link ? TouchableOpacity : View;
  const touchableProps = link
    ? {
        onPress,
        activeOpacity: 0.8,
      }
    : {};

  return (
    <Container
      style={styles.container}
      {...touchableProps}
    >
      <View style={styles.iconContainer}>
        <Ionicons name={icon} size={21} color={colors.purplePrimary} />
      </View>

      <Text
        style={[
          styles.value,
          link && styles.link,
        ]}
      >
        {value}
      </Text>

      {link && (
        <View style={styles.externalContainer}>
          <Ionicons name="open-outline" size={18} color={colors.grayText} />
        </View>
      )}
    </Container>
  );
}

const styles = StyleSheet.create({
  container: {
    minHeight: 72,
    borderWidth: 1,
    borderColor: colors.grayLight,
    borderRadius: radius.md,
    backgroundColor: colors.white,
    paddingHorizontal: spacing.md,
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },

  iconContainer: {
    width: 40,
    height: 40,
    borderRadius: radius.pill,
    backgroundColor: colors.purpleSoft,
    alignItems: 'center',
    justifyContent: 'center',
  },

  value: {
    flex: 1,
    fontSize: typography.size.md,
    lineHeight: typography.lineHeight.md,
    color: colors.grayDark,
  },

  link: {
    color: colors.purplePrimary,
  },

  externalContainer: {
    width: 32,
    height: 32,
    borderRadius: radius.pill,
    backgroundColor: colors.whiteSoft,
    alignItems: 'center',
    justifyContent: 'center',
  },
});
