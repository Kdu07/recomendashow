import React from 'react';
import { Image, StyleSheet, Text, TouchableOpacity, View } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function TheaterCard({
  name,
  location,
  image,
  onPress,
  width = 340,
}) {
  return (
    <TouchableOpacity
      style={[styles.card, { width }]}
      onPress={onPress}
      activeOpacity={0.85}
    >
      <Image source={image} style={styles.image} resizeMode="cover" />

      <View style={styles.content}>
        <Text style={styles.name} numberOfLines={1}>
          {name}
        </Text>

        {location && (
          <Text style={styles.location} numberOfLines={1}>
            {location}
          </Text>
        )}
      </View>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  card: {
    alignSelf: 'center',
    borderRadius: radius.lg,
    overflow: 'hidden',
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
  },

  image: {
    width: '100%',
    height: 150,
    backgroundColor: colors.purpleSoft,
  },

  content: {
    padding: spacing.md,
    gap: spacing.xs,
    alignItems: 'center',
    justifyContent: 'center',
  },

  name: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
    textAlign: 'center',
  },

  location: {
    fontSize: typography.size.sm,
    color: colors.grayText,
    textAlign: 'center',
  },
});