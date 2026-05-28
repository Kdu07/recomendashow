import React from 'react';
import { Image, Pressable, StyleSheet, Text, View } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing, radius } from '../constants/spacing';

export default function SearchResultItem({
  title,
  location,
  category,
  image,
  onPress,
}) {
  return (
    <Pressable
      style={({ pressed }) => [
        styles.container,
        pressed && styles.pressed,
      ]}
      onPress={onPress}
    >
      <Image source={image} style={styles.image} resizeMode="cover" />

      <View style={styles.content}>
        <Text style={styles.title} numberOfLines={1}>
          {title}
        </Text>

        <Text style={styles.location} numberOfLines={1}>
          ⊕ {location}
        </Text>

        <Text style={styles.category} numberOfLines={1}>
          {category}
        </Text>
      </View>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
    paddingVertical: spacing.md,
    borderBottomWidth: 1,
    borderBottomColor: colors.grayLight,
  },

  pressed: {
    opacity: 0.82,
    transform: [{ scale: 0.99 }],
  },

  image: {
    width: 72,
    height: 72,
    borderRadius: radius.md,
    backgroundColor: colors.purpleSoft,
  },

  content: {
    flex: 1,
    gap: 4,
  },

  title: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  location: {
    fontSize: typography.size.sm,
    color: colors.grayText,
  },

  category: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.peachBurnt,
  },
});